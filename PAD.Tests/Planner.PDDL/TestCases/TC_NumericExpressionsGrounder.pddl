(define (domain domainName)
  (:requirements :typing :adl :fluents)
  (:constants constA constB)
  (:functions (numFunc ?a) - number)
  (:action actionName0
    :parameters (?a ?b)
    :precondition (< (numFunc ?a) (+ (numFunc ?b) 5 6 8))
  )
)
