(define (domain domainName)
  (:requirements :preferences :adl :numeric-fluents)
  (:predicates (predA ?a) (predB ?a))
  (:action actionA
    :parameters (?a ?b)
	:precondition (and
	                (preference prefName (predA ?a))
                    (predB ?a)
					(= ?a ?b)
					(or )
					(not (predA ?a))
					(imply
					  (predA ?a)
					  (predB ?a)
					)
					(exists (?c) (predA ?a))
					(forall (?c) (predB ?a))
					(< 5 6)
                  )
    :effect ()
  )
)