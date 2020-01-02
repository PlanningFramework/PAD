(define (domain domainName)
  (:requirements :typing :adl :fluents)
  (:types typeA typeB)
  (:constants constA constB)
  (:predicates (predA ?a ?b))
  (:functions (objFunc ?a) - object
              (numFunc ?a) - number
  )
  (:action actionName0
    :parameters (?a ?b)
    :precondition (and
                    (predA ?a ?b)
                    (= (objFunc ?a) ?b)
                    (< (numFunc ?a) 5)
                    (forall (?c) (predA ?a ?c))
                  )
  )
)
