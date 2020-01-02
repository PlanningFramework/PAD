(and
  (at start (pred ?aa))
  (forall (?aa) (at start (pred ?aa)))
  (when
    (at start (pred ?aa))
    (at end (pred ?aa))
  )
)