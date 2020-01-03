(and
  (at start (pred ?aa))
  (at end (and
            (predA ?aa)
            (predB ?bb)
          )
  ) 
  (increase (func ?aa) #t)
  (decrease funcB (* #t 3))
)