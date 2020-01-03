(define (domain domainName)
  (:requirements :typing :adl :fluents)
  (:types typeA - typeB
          typeC
  )
  (:constants constA constB - typeA
              constC - typeC
  )
  (:predicates (pred1 ?a ?b)
               (pred2 ?a)
  )
  (:functions (objFunc ?a) - object
              (numFunc ?a) - number
  )
  (:action actionName0
    :parameters (?a ?b - typeA ?c - typeC)
    :precondition (and
                    (pred1 ?a ?c)
                    (pred2 (objFunc ?a))
                    (< (numFunc ?b) (numFunc ?a))
                  )
  )
)
