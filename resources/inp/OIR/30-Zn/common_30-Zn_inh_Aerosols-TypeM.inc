[include:common_30-Zn_parameter.inc]

[parameter]
  # ICRP Publ.134 p.186 Table 9.2 / Inhaled particulate materials, Absorption type M

  $fr = 0.2
  $sr = 3
  $ss = 0.005
  $fb = 0           # footnote*
  $sb = 0           # footnote*

$if parent
  ${Zn}/fA    = 0.1
  ${Zn}/fA_Re = fA_MaxValueOfIng    # footnote¶
$else
  ${^Cu}/fA    = fr * fA_MaxValueOfIng
  ${^Cu}/fA_Re =      fA_MaxValueOfIng
$endif

[include:common_intake_inh.inc]

[include:common_HRTM.inc]
[include:common_HATM+UB.inc]

[include:element_31-Ga.inc]
[include:element_30-Zn.inc]
[include:element_29-Cu.inc]
