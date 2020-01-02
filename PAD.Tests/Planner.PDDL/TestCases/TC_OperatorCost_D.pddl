(define (domain domainName)
  (:requirements :action-costs)
  (:predicates (pred0) (pred1))
  (:functions (total-cost) - number)
  (:action actionName0
    :parameters ()
    :precondition ()
    :effect (and (pred0)
                 (not (pred1))
                 (increase (total-cost) 4)
            )
  )
  (:action actionName1
    :parameters ()
    :precondition (pred1)
    :effect (increase total-cost 7)
  )
)
