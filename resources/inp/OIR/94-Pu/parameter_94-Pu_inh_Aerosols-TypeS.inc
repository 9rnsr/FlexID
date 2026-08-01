[include:parameter_94-Pu_common.inc]

[parameter]
  # ICRP Publ.141 p.337 Table 22.11 / Inhaled particulate materials, Absorption type S

  $fr = 0.01
  $sr = 0.4
  $ss = 1E-4
  $fb = 0.002       # footnote*
  $sb = 0           # footnote*

$if parent
  ${Pu}/fA     = 5E-6
  ${Pu}/fA_Re  = fA_MaxValueOfIng   # footnote§
$else
  ${^Rn}/fA    = fr * fA_MaxValueOfIng
  ${^Rn}/fA_Re =      fA_MaxValueOfIng
$endif
