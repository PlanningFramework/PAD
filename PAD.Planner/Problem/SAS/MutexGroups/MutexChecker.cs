
namespace PAD.Planner.SAS
{
    /// <summary>
    /// Implementation of the mutex checker. Provides methods for checking operators applicability with regards to defined mutex groups in the
    /// SAS+ planning problem.
    /// </summary>
    public class MutexChecker
    {
        /// <summary>
        /// Currently locked reference state.
        /// </summary>
        private IState LockedState { set; get; } = null;

        /// <summary>
        /// Current locks for each mutex group corresponding to the reference state. E.g. 'StateLocks[0] = 2' means that the first mutex
        /// group has a lock on the item with index 2.
        /// </summary>
        private int[] CachedStateLocks { set; get; } = null;

        /// <summary>
        /// Current locks for each mutex group. Cannot lock an item which is already locked.
        /// </summary>
        private int[] Locks { set; get; } = null;

        /// <summary>
        /// Mutex groups of the SAS+ planning problem.
        /// </summary>
        private MutexGroups MutexGroups { set; get; } = null;

        /// <summary>
        /// Constructs the mutex checker.
        /// </summary>
        /// <param name="mutexGroups">Mutex groups of the SAS+ planning problem.</param>
        public MutexChecker(MutexGroups mutexGroups)
        {
            MutexGroups = mutexGroups;
            CachedStateLocks = new int[mutexGroups.Count];
            Locks = new int[mutexGroups.Count];

            ReleaseCachedStateLocks();
            ReleaseLocks();
        }

        /// <summary>
        /// Checks whether the given state complies with mutex constraints in the SAS+ planning problem.
        /// </summary>
        /// <param name="state">State to be checked.</param>
        /// <returns>True, if the state complies with mutex constraints, false otherwise.</returns>
        public bool CheckState(IState state)
        {
            return LockState(state, true);
        }

        /// <summary>
        /// Checks whether the given conditions complies with mutex constraints in the SAS+ planning problem.
        /// </summary>
        /// <param name="conditions">Conditions to be checked.</param>
        /// <returns>True, if the conditions complies with mutex constraints, false otherwise.</returns>
        public bool CheckConditions(IConditions conditions)
        {
            return MutexGroups.TrueForAll(mutexGroup => conditions.IsCompatibleWithMutexContraints(mutexGroup));
        }

        /// <summary>
        /// Checks whether the potential successor given by the application of the specified operator to the specified state is compatible
        /// with the mutex constraints of the SAS+ planning problem.
        /// </summary>
        /// <param name="state">Reference state (we assume it complies with the mutex constraints).</param>
        /// <param name="oper">Operator to be checked.</param>
        /// <returns>True, if the operator is applicable with regards to defined mutex groups, false otherwise.</returns>
        public bool CheckSuccessorCompatibility(IState state, IOperator oper)
        {
            // forward compatibility is optimized for continuous checking of one state with multiple operators
            return LockState(state, false) && LockForwardOperator(oper);
        }

        /// <summary>
        /// Checks whether the potential predecessor given by the backwards application of the specified operator to the specified conditions
        /// is compatible with the mutex constraints of the SAS+ planning problem.
        /// </summary>
        /// <param name="conditions">Reference conditions.</param>
        /// <param name="oper">Operator to be checked.</param>
        /// <returns>True, if the operator is backwards-applicable with regards to defined mutex groups, false otherwise.</returns>
        public bool CheckPredecessorCompatibility(IConditions conditions, IOperator oper)
        {
            // actually quite efficient solution because anything else would be even worse; unfortunately, backwards compatibility is very tricky
            return CheckConditions((IConditions)oper.ApplyBackwards(conditions));
        }

        /// <summary>
        /// Evaluates the given state for compliance with mutex groups. Goes through the state and locks all active mutexes.
        /// </summary>
        /// <param name="state">Reference state.</param>
        /// <param name="fullEvaluation">In case of partial elevation, we are sure the state is correct and we find only the 1st active mutex in each group.</param>
        /// <returns>True, if the state complies with mutex groups, false otherwise.</returns>
        private bool LockState(IState state, bool fullEvaluation)
        {
            if (state.Equals(LockedState))
            {
                return true;
            }
            LockedState = null;
            ReleaseCachedStateLocks();

            if (!DoLockStateMutexes(state, fullEvaluation))
            {
                ReleaseCachedStateLocks();
                return false;
            }

            LockedState = state;
            return true;
        }

        /// <summary>
        /// Internal method for locking of mutexes, based on a checking function and a locking function doing the actual job.
        /// </summary>
        /// <param name="state">Reference state.</param>
        /// <param name="fullEvaluation">In case of partial elevation, we are sure the state is correct and we find only the 1st active mutex in each group.</param>
        /// <returns>True, if the mutexes were successfully locked, false otherwise.</returns>
        private bool DoLockStateMutexes(IState state, bool fullEvaluation)
        {
            // Find active mutexes in the given state and lock them
            for (int groupIndex = 0; groupIndex < MutexGroups.Count; ++groupIndex)
            {
                var mutexGroup = MutexGroups[groupIndex];
                for (int itemIndex = 0; itemIndex < mutexGroup.Count; ++itemIndex)
                {
                    var mutexItem = mutexGroup[itemIndex];
                    if (state.HasValue(mutexItem))
                    {
                        if (!TryLockCachedStateMutex(groupIndex, itemIndex))
                        {
                            return false;
                        }

                        // In partial evaluation, we assume the state are correct and there are no more active mutexes in the group
                        if (!fullEvaluation)
                        {
                            break;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Evaluates the application of the given operator for compliance with mutex groups. We assume the corresponding reference state
        /// has been already set (via LockState). Goes through the operator effects and locks all active mutexes.
        /// </summary>
        /// <param name="oper">Operator to be checked.</param>
        /// <returns>True, if the operator is applicable with compliance with the mutex groups, false otherwise.</returns>
        private bool LockForwardOperator(IOperator oper)
        {
            InitLocksByState();

            for (int groupIndex = 0; groupIndex < MutexGroups.Count; ++groupIndex)
            {
                if (IsGroupLocked(groupIndex))
                {
                    foreach (var effect in oper.GetEffects())
                    {
                        if (effect.IsApplicable(LockedState)) // effect can be conditional
                        {
                            // if any effect alters the currently locked item, then we need to unlock it
                            if (TryUnlockAlteredMutexItem(groupIndex, effect.GetAssignment()))
                            {
                                break;
                            }
                        }
                    }
                }

                var mutexGroup = MutexGroups[groupIndex];
                foreach (var effect in oper.GetEffects())
                {
                    if (effect.IsApplicable(LockedState)) // effect can be conditional
                    {
                        int itemIndex = -1;
                        if (mutexGroup.TryFindAffectedMutexItem(effect.GetAssignment(), out itemIndex))
                        {
                            if (!TryLockMutex(groupIndex, itemIndex))
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Tries to lock the specified reference state mutex item in the given mutex group.
        /// </summary>
        /// <param name="groupIdx">Mutex group index.</param>
        /// <param name="itemIdx">Mutex item index.</param>
        /// <returns>True, if the specified item has been successfully locked, false otherwise (already locked).</returns>
        private bool TryLockCachedStateMutex(int groupIndex, int itemIndex)
        {
            if (CachedStateLocks[groupIndex] != -1)
            {
                return false;
            }
            CachedStateLocks[groupIndex] = itemIndex;
            return true;
        }

        /// <summary>
        /// Tries to lock the specified operator mutex item in the given mutex group.
        /// </summary>
        /// <param name="groupIndex">Mutex group index.</param>
        /// <param name="itemIndex">Mutex item index.</param>
        /// <returns>True, if the specified item has been successfully locked (or had been locked before). False if locking failed.</returns>
        private bool TryLockMutex(int groupIndex, int itemIndex)
        {
            int lockedGroupItem = Locks[groupIndex];
            if (lockedGroupItem != -1)
            {
                if (lockedGroupItem == itemIndex)
                {
                    // requested item is already locked -> no problem
                    return true;
                }

                var mutexGroup = MutexGroups[groupIndex];
                if (mutexGroup[lockedGroupItem].GetVariable() == mutexGroup[itemIndex].GetVariable())
                {
                    // new asignment on the same variable (previously locked by state) -> continue and rewrite the previous one
                }
                else
                {
                    // already locked on a different item -> violation of group constraints
                    return false;
                }
            }

            Locks[groupIndex] = itemIndex;
            return true;
        }

        /// <summary>
        /// Checks whether the given mutex group is already locked.
        /// </summary>
        /// <param name="groupIndex">Mutex group index.</param>
        /// <returns>True, if the specified item is locked, false otherwise.</returns>
        private bool IsGroupLocked(int groupIndex)
        {
            return (Locks[groupIndex] != -1);
        }

        /// <summary>
        /// Tries to unlocked the specified mutex item, if the assignment changes the value of currently locked item.
        /// </summary>
        /// <param name="groupIndex">Mutex group index.</param>
        /// <param name="assignment">Effect assignment.</param>
        /// <returns>True, if the specified group item was unclocked, false otherwise.</returns>
        private bool TryUnlockAlteredMutexItem(int groupIndex, IAssignment assignment)
        {
            int lockedItem = Locks[groupIndex];
            if (lockedItem == -1)
            {
                return true;
            }

            var mutexGroup = MutexGroups[groupIndex];
            if (assignment.GetVariable() == mutexGroup[lockedItem].GetVariable())
            {
                Locks[groupIndex] = -1;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to lock the specified operator mutex item in the given mutex group.
        /// </summary>
        /// <param name="groupIndex">Mutex group index.</param>
        /// <param name="itemIndex">Mutex item index.</param>
        /// <returns>True, if the specified item has been successfully locked (or had been locked before). False otherwise.</returns>
        private void UnlockOperatorMutex(int groupIndex, int itemIndex)
        {
            int lockedGroupItem = Locks[groupIndex];
            if (lockedGroupItem == itemIndex)
            {
                Locks[groupIndex] = -1;
            }
        }

        /// <summary>
        /// Releases all locks of all mutex groups corresponding to the reference state.
        /// </summary>
        private void ReleaseCachedStateLocks()
        {
            for (int i = 0; i < CachedStateLocks.Length; ++i)
            {
                CachedStateLocks[i] = -1;
            }
        }

        /// <summary>
        /// Releases all locks of all mutex groups.
        /// </summary>
        private void ReleaseLocks()
        {
            for (int i = 0; i < Locks.Length; ++i)
            {
                Locks[i] = -1;
            }
        }

        /// <summary>
        /// Initialize the locks from the previously cached state locks.
        /// </summary>
        private void InitLocksByState()
        {
            CachedStateLocks.CopyTo(Locks, 0);
        }
    }
}
