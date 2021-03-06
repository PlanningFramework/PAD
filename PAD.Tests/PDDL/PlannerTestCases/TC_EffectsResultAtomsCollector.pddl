(define (domain domainName)
  (:requirements :adl :fluents)
  (:constants constA constB)
  (:predicates (predA) (predB) (predC ?a) (predD ?a) (predE))
  (:functions (numericFunc) - number
              (objectFunc) - object
  )
  (:action actionName
    :parameters (?a)
    :precondition ()
    :effect (and
              (predA)
              (not (predB))
              (predC ?a)
              (forall (?x) (predD ?x))
              (assign (numericFunc) 5)
              (increase (numericFunc) 5)
              (decrease (numericFunc) 5)
              (scale-up (numericFunc) 5)
              (scale-down (numericFunc) 5)
              (assign (objectFunc) constA)
              (when (predA) (predE))
            )
  )
)
