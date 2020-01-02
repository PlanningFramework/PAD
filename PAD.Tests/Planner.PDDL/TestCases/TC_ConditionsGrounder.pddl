(define (domain domainName)
  (:requirements :adl)
  (:constants constA constB)
  (:predicates (pred ?a ?b))
  (:action actionName0
    :parameters (?a ?b)
    :precondition (pred ?a ?b)
  )
)
