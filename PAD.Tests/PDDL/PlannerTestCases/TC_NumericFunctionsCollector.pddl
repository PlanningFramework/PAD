(define (domain domainName)
  (:requirements :typing :adl :fluents)
  (:constants constA constB)
  (:functions (numFuncA ?a ?b) (numFuncB) (numFuncC ?a) - number)
  (:action actionName0
    :parameters (?a)
    :precondition (<
                    (+
                      (/ numFuncB numFuncB)
                      (- (numFuncA constA ?a))
                      (* 4 (numFuncC constB))
                    )
                    8
                  )
  )
)
