(define (domain domainName)
  (:requirements :typing :adl :fluents)
  (:constants constA constB)
  (:predicates (predA) (predB) (predC) (predD ?a))
  (:action actionName0
    :parameters ()
    :precondition (and
                    (predA)
                    (predB)
                    (predC)
                  )
  )
  (:action actionName1
    :parameters (?a)
    :precondition (and
                    (not (not (predA)))
                    (or (predB) (predC))
                    (predD ?a)
                  )
  )
)
