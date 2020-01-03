(define (domain domainName)
  (:requirements :typing :adl)
  (:types typeA typeB typeC)
  (:constants constA constB)
  (:predicates (pred ?a))
  (:action actionName0
    :parameters (?a - typeA ?b - typeB)
    :precondition (pred ?a)
  )
  (:action actionName1
    :parameters (?a - (either typeB typeC))
    :precondition (pred ?a)
  )
)
