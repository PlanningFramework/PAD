(define (domain domainName)
  (:requirements :adl)
  (:constants constA constB)
  (:predicates (predA) (predB) (predC ?a))
  (:action actionName0
    :parameters (?a)
    :precondition (and (predA)
                       (not (predB))
                       (or (predC ?a)
                           (= constA ?a)
                       )
                       (imply (predC constB) (predA))
                  )
  )
)