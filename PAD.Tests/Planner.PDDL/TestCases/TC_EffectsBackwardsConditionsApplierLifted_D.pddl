(define (domain domainName)
  (:requirements :adl :fluents)
  (:types typeX typeY)
  (:constants x1 x2 - typeX y1 y2 - typeY)
  (:predicates (predStart) (pred ?x - object))
  (:action first
    :parameters (?x - typeX)
    :precondition (predStart)
    :effect (and
              (not (predStart))
              (pred ?x)
            )
  )
  (:action change
    :parameters (?x - typeX ?y - typeY)
    :precondition (pred ?x)
    :effect (and
              (not (pred ?x))
              (pred ?y)
            )
  )
)
