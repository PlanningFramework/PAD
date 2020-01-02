(define (domain domainName)
  (:requirements :typing :adl :fluents)
  (:types typeA - typeB)
  (:constants constA constB)
  (:predicates (predA) (predB ?a) (predB ?a ?b))
  (:functions (objFunc ?a) - object
              (numFunc ?a) - number
  )
  (:action actionName0
    :parameters (?a ?b)
    :precondition ()
    :effect ()
  )
)
