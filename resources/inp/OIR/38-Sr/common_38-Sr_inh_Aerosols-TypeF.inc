[include:common_38-Sr_parameter.inc]

[parameter]
  # ICRP Publ.134 p.215 Table 10.2 / Inhaled particulate materials, Absorption type F

  $fr = 1
  $sr = 30
  $ss =  0
  $fb = 0           # footnote*
  $sb = 0           # footnote*

$if parent
  ${Sr}/fA    = 0.25
  ${Sr}/fA_Re = 0
$else
  ${*}/fA     = fr * fA_MaxValueOfIng
  ${*}/fA_Re  =      fA_MaxValueOfIng
$endif

[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_39-Y.inc]
[include:element_38-Sr.inc]
[include:element_37-Rb.inc]
[include:element_36-Kr.inc]
