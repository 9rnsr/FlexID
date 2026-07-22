[include:parameter_90-Th_common.inc]

[parameter]
  # ICRP Publ.137 p.362 Table 14.2 / Inhaled particulate materials, Absorption type F

  $fr = 1
  $sr = 50
  $ss = 0
  $fb = 0           # footnote*
  $sb = 0           # footnote*

$if parent
  ${Th}/fA     = 5E-4
  ${Th}/fA_Re  = fA_MaxValueOfIng   # footnote**
$else
  ${^Rn}/fA    = fr * fA_MaxValueOfIng
  ${^Rn}/fA_Re =      fA_MaxValueOfIng
$endif
