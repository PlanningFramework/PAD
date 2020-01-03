(define (domain domainName)
  (:requirements :typing :adl :fluents)
  (:types typeA typeB)
  (:constants constA constB)
  (:predicates (predA) (predB) (predC) (predD) (predE) (predF) (predG ?a))
  (:functions (objFunc) - object)
  (:action actionName0
    :parameters ()
    :precondition (or (or (predA) (or (predB) (predC)) (predB) (predC)) (predD))
  )
  (:action actionName1
    :parameters ()
    :precondition (and (and (predA) (and (predB) (predC)) (predB) (predC)) (predD))
  )
  (:action actionName2
    :parameters ()
    :precondition (or (and
                         (predA)
                         (predB)
                       )
                       (predE)
                       (and
                          (predC)
                          (predD)
                       )
                  )       
  )
  (:action actionName3
    :parameters ()
    :precondition (not (imply (predA) (predB)))
  )
  (:action actionName4
    :parameters ()
    :precondition (forall (?a) (and (not (predA)) (predG ?a)))
  )
  (:action actionName5
    :parameters ()
    :precondition (exists (?a) (predG ?a))
  )
)
