[include:common_82-Pb_parameter.inc]

[parameter]
  # ICRP Publ.137 p.220 Table 9.2 / Inhaled particulate materials, Absorption type F

  $fr = 1
  $sr = 100
  $ss = 0
  $fb = 0.5         # footnote*
  $sb = 1.7         # footnote*

$if parent
  ${Pb}/fA    = 0.2
  ${Pb}/fA_Re = fA_MaxValueOfIng    # footnote**
$else
  ${*}/fA     = fr * fA_MaxValueOfIng
  ${*}/fA_Re  =      fA_MaxValueOfIng
$endif

[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_84-Po.inc]
[include:element_83-Bi.inc]
[include:element_82-Pb.inc]
[include:element_81-Tl.inc]
[include:element_80-Hg.inc]
[include:element_79-Au.inc]
