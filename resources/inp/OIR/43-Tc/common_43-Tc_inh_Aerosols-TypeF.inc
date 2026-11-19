[include:common_43-Tc_parameter.inc]

[parameter]
  # ICRP Publ.134 p.322 Table 15.2 / Inhaled particulate materials, Absorption type F

  $fr = 1
  $sr = 100
  $ss =   0
  $fb = 0           # footnote*
  $sb = 0           # footnote*

$if parent
  ${Tc}/fA    = 0.9
  ${Tc}/fA_Re = fA_MaxValueOfIng    # footnote¶
$else
  ${*}/fA     = fr * fA_MaxValueOfIng
  ${*}/fA_Re  =      fA_MaxValueOfIng
$endif

[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_43-Tc.inc]
