(and
  (at start (<= ?duration 33))
  (at end (>= ?duration func))
  (<= ?duration (+ 3 5))
  (= ?duration (func ?aa))
)