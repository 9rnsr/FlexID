[include:common_82-Pb_parameter.inc]

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

[include:common_intake_inh.inc]

[include:common_HRTM.inc]
[include:common_HATM+UB.inc]

[include:element_84-Po.inc]
[include:element_83-Bi.inc]
[include:element_82-Pb.inc]
[include:element_81-Tl.inc]
[include:element_80-Hg.inc]
[include:element_79-Au.inc]
