(and
  (pred ?aa)
  (increase func 8)
  (forall (?aa) (pred ?aa))
  (when (pred ?bb)
    (and
      (predA ?aa)
      (predB ?aa)
    )
  )
)