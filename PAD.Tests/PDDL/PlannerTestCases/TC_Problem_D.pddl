(define (domain domainName)
  (:requirements :typing :adl :fluents)
  (:constants constA constB)
  (:predicates (pred0) (pred1) (pred2) (predRigid))
  (:action actionName0
    :parameters ()
    :precondition (pred0)
    :effect (and
              (pred1)
              (not (pred2))
            )
  )
  (:action actionName1
    :parameters ()
    :precondition ()
    :effect (pred0)
  )
)
