(define (problem problemName)
  (:domain domainName)
  (:goal
    (and
      (preference prefName (predA constA))
      (predA constA)
      (= constA constB)
      (or )
      (not (predA constA))
      (imply
        (predA constA)
       	(predB constA)
      )
      (exists (?a) (predA ?a))
      (forall (?a) (predB ?a))
      (< 5 6)
    )
  )
)