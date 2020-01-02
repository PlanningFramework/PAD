(define (domain domainName)
  (:requirements :typing :fluents :equality)
  (:functions
    (numFuncA ?a) (numFuncB) - number
	(objFuncA) (objFuncB ?a) - object
  )
  (:action actionA
    :parameters (?a ?b - object)
	:precondition (and
	                (= objFuncA (objFuncB ?b))
					(= (objFuncA) ?b)
					(= (numFuncA ?a) numFuncB)
					(< (numFuncB) 6)
					(<= (+ (+ 9 4) 3 2) (- 1 1))
					(> (* 5 9) (- numFuncB))
					(>= 3 (/ 9 3))
                  )
    :effect ()
  )
)