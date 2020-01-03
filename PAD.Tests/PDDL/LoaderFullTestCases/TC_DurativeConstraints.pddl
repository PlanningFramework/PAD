(define (domain domainName)
  (:requirements :durative-actions :duration-inequalities :numeric-fluents)
  (:functions (func) - number)
  (:durative-action actionA
    :parameters ()
    :duration (and
	            (at start (<= ?duration 5))
                (at end (= ?duration (func)))
				(>= ?duration (+ 2 3))
	          )
	:condition ()
    :effect ()
  )
)
