(define (domain domainName)
  (:requirements :typing :adl :fluents)
  (:constants constA constB)
  (:predicates (pred0) (pred1 ?a ?b ?c))
  (:functions (objFunc) - object
              (numFunc ?a) - number
  )
  (:action actionName0
    :parameters (?a ?b ?c)
    :precondition (and
                    (pred0)
                    (pred1 ?a constA (objFunc))
                    (= (objFunc) constA)
                    (= (numFunc constA) 0)
                    (pred1 ?a ?b ?c)
                  )
  )
)
