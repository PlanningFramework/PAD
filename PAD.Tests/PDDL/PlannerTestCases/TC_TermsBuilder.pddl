(define (domain domainName)
  (:requirements :typing :adl :fluents)
  (:constants constA constB)
  (:predicates (pred ?a))
  (:functions (objFunc ?a) - object)
  (:action actionName0
    :parameters (?a ?b)
    :precondition (and
                    (pred constA)
                    (= ?a ?b)
                    (= (objFunc ?a) constB)
                  )
  )
)
