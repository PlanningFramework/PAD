(define (domain domainName)
  (:requirements :adl :fluents)
  (:constants constA constB)
  (:predicates (rigidPred) (predA) (predB) (predC) (predD) (predE) (predF ?a ?b) (predZ) (pred1) (pred2))
  (:functions (objFunc1) (objFunc2) - object
              (numFunc1) (numFunc2) (numFunc3) - number
  )
  (:action actionName0
    :parameters ()
    :precondition (and (rigidPred) (predZ) (not (pred1)))
    :effect (and
              (predA)
              (not (predB))
              (predC)
            )
  )
  (:action actionName1
    :parameters (?a)
    :precondition ()
    :effect (assign (objFunc1) constA)
  )
  (:action actionName2
    :parameters ()
    :precondition ()
    :effect (and
              (assign (numFunc1) 3)
              (increase (numFunc3) 10)
            )
  )
  (:action actionName3
    :parameters (?a)
    :precondition ()
    :effect (and
              (predA)
              (forall (?b) (predF ?a ?b))
            )
  )
  (:action actionName4
    :parameters ()
    :precondition (predZ)
    :effect (and
              (predC)
              (when (pred1) (predA))
              (when (pred2) (predB))
            )
  )
)
