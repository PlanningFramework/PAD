(define (domain domainName)
  (:requirements :typing :adl :fluents)
  (:constants constA)
  (:predicates (pred))
  (:functions (numericFunc) - number
              (objectFunc) - object
  )
  (:action actionName
    :parameters ()
    :precondition ()
    :effect (and
              (pred)
              (not (pred))
              (forall (?x) (pred))
              (assign (numericFunc) 5)
              (increase (numericFunc) 5)
              (decrease (numericFunc) 5)
              (scale-up (numericFunc) 5)
              (scale-down (numericFunc) 5)
              (assign (objectFunc) constA)
              (when (pred) (not (pred)))
            )
  )
)
