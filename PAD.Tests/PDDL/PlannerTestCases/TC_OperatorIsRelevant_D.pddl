(define (domain domainName)
  (:requirements :typing :adl :fluents)
  (:constants constA constB)
  (:predicates (predA) (predB) (predC) (predD) (predE) (predF ?a)
               (predG ?a) (predH) (predI ?a) (predJ) (predK ?a)
               (predL) (predM) (predN)
  )
  (:functions (numFunc) (numFuncB) - number
              (objFunc) - object
  )
  (:action actionName0
    :parameters ()
    :precondition ()
    :effect (and (predA) (predB) (not (predE)))
  )
  (:action actionName1
    :parameters ()
    :precondition ()
    :effect (and (predA) (not (predB)))
  )
  (:action actionName2
    :parameters ()
    :precondition ()
    :effect (and (predC) (not (predD)))
  )
  (:action actionName3
    :parameters ()
    :precondition ()
    :effect (and (not (predC)) (not (predD)))
  )
  (:action actionName4
    :parameters ()
    :precondition ()
    :effect (increase (numFunc) 10)
  )
  (:action actionName5
    :parameters ()
    :precondition ()
    :effect (assign (objFunc) constA)
  )
  (:action actionName6
    :parameters ()
    :precondition ()
    :effect (assign (numFuncB) 10)
  )
  (:action actionName7
    :parameters (?a)
    :precondition ()
    :effect (predF ?a)
  )
  (:action actionName8
    :parameters (?a)
    :precondition ()
    :effect (predG ?a)
  )
  (:action actionName9
    :parameters (?a)
    :precondition ()
    :effect (and (predG ?a) (not (predH)))
  )
  (:action actionName10
    :parameters (?a)
    :precondition ()
    :effect (and (predJ) (not (predI ?a)))
  )
  (:action actionName11
    :parameters (?a)
    :precondition ()
    :effect (and (not (predJ)) (predI ?a))
  )
  (:action actionName12
    :parameters ()
    :precondition ()
    :effect (forall (?a) (predK ?a))
  )
  (:action actionName13
    :parameters ()
    :precondition ()
    :effect (and
              (predL)
              (when (predA) (predL))
              (when (predA) (predM))
              (when (predA) (not (predM)))
              (when (predA) (and
                              (predL)
                              (predM)
                            )
              )
            )
  )
  (:action actionName14
    :parameters ()
    :precondition ()
    :effect (when (predA) (predN))
  )
  (:action actionName15
    :parameters ()
    :precondition ()
    :effect (when (predA) (predL))
  )
  (:action actionName16
    :parameters ()
    :precondition ()
    :effect (and
              (not (predL))
              (when (predA) (predL))
            )
  )
)
