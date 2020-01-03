(define (domain domainName)
  (:requirements :typing :adl :fluents)
  (:constants constA constB)
  (:predicates (predA ?a ?b) (predB ?a))
  (:functions (objFunc ?a) - object
              (numFunc ?a) - number
  )
  (:action actionName0
    :parameters (?a ?b)
    :precondition (and
                    (predA ?a ?b)
                    (predB (objFunc ?a))
                    (< (numFunc ?a) (numFunc ?b))
                  )
  )
)
