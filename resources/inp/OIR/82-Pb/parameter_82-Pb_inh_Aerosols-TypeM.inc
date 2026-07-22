[include:parameter_82-Pb_common.inc]

[parameter]
  # ICRP Publ.137 p.220 Table 9.2 / Inhaled particulate materials, Absorption type M

  $fr = 0.2
  $sr = 3
  $ss = 0.005
  $fb = 0.5         # footnote*
  $sb = 1.7         # footnote*

$if parent
  ${Pb}/fA    = 0.04
  ${Pb}/fA_Re = fA_MaxValueOfIng    # footnote**
$else
  ${*}/fA     = fr * fA_MaxValueOfIng
  ${*}/fA_Re  =      fA_MaxValueOfIng
$endif
