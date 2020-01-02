(define (domain domainName)
  (:requirements :typing :adl)
  (:types typeA typeB)
  (:constants constA constB)
  (:predicates (pred ?a))
  (:action actionName0
    :parameters (?a - typeA)
    :precondition (pred ?a)
  )
  (:action actionName1
    :parameters (?a - typeB)
    :precondition (pred ?a)
  )
)
