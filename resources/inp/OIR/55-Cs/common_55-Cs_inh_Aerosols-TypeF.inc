[include:common_55-Cs_parameter.inc]

[parameter]
  # ICRP Publ.137 p.142 Table 6.2 / Inhaled particulate materials, Absorption type F

  $fr = 1
  $sr = 100
  $ss =   0
  $fb = 0           # footnote*
  $sb = 0           # footnote*

$if parent
  ${Cs}/fA    = 0.99 # ≒1
  ${Cs}/fA_Re = fA_MaxValueOfIng    # footnote¶
$else
  ${*}/fA     = fr * fA_MaxValueOfIng
  ${*}/fA_Re  =      fA_MaxValueOfIng
$endif

[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_56-Ba.inc]
[include:element_55-Cs.inc]
