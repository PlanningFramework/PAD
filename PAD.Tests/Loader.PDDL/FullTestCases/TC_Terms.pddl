(define (domain domainName)
  (:requirements :typing)
  (:types typeA typeB)
  (:predicates (predicateA
                 ?a - (either typeA typeB)
                 ?b ?c - typeB
                 ?d - (either typeA)
                 ?e
               )
  )
)