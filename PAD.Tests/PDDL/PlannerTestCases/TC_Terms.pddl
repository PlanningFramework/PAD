(define (domain domainName)
  (:requirements :typing :adl :fluents)
  (:constants constA)
  (:functions (objFunc ?a) - object)
  (:action actionName0
    :parameters (?a ?b)
    :precondition (and
                    (= ?a constA)
                    (= (objFunc ?a) ?b)
                  )
  )
)
