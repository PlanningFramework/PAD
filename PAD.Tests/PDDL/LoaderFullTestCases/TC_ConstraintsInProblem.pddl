(define (problem problemName)
  (:domain domainName)
  (:constraints
    (and
      (forall (?a) (always (pred ?a)))
      (at end (pred constA))
      (always (pred constA))
      (sometime (pred constA))
      (within 6 (pred constA))
      (at-most-once (pred constA))
      (sometime-after (pred constA) (pred constA))
      (sometime-before (pred constA) (pred constA))
      (always-within 6 (pred constA) (pred constA))
      (and
        (and
          (hold-during 6 6 (pred constA))
          (hold-after 6 (pred constA))
        )
      )
    )
  )
)