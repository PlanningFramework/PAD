(define (domain domainName)
  (:requirements :typing :adl :fluents)
  (:constants constA constB)
  (:predicates (pred0) (pred1 ?a))
  (:functions (numericFunc) - number
              (objectFunc) - object
  )
  (:action actionName0
    :parameters ()
    :precondition ()
    :effect (pred1 constB)
  )
  (:action actionName1
    :parameters ()
    :precondition ()
    :effect (not (pred0))
  )
  (:action actionName2
    :parameters ()
    :precondition ()
    :effect (forall (?x) (not (pred1 ?x)))
  )
  (:action actionName3
    :parameters ()
    :precondition ()
    :effect (assign (numericFunc) 5)
  )
  (:action actionName4
    :parameters ()
    :precondition ()
    :effect (increase (numericFunc) 7)
  )
  (:action actionName5
    :parameters ()
    :precondition ()
    :effect (decrease (numericFunc) 2)
  )
  (:action actionName6
    :parameters ()
    :precondition ()
    :effect (scale-up (numericFunc) 3)
  )
  (:action actionName7
    :parameters ()
    :precondition ()
    :effect (scale-down (numericFunc) 2)
  )
  (:action actionName8
    :parameters ()
    :precondition ()
    :effect (assign (objectFunc) constB)
  )
  (:action actionName9
    :parameters ()
    :precondition ()
    :effect (when (not (pred0)) (assign (numericFunc) 0))
  )
  (:action actionName10
    :parameters ()
    :precondition ()
    :effect (when (pred0) (assign (numericFunc) 1))
  )
  (:action actionName11
    :parameters (?a)
    :precondition ()
    :effect (pred1 ?a)
  )
)
