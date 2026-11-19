[include:common_78-Pt_parameter.inc]

[parameter]
  # ICRP Publ.151 p.305 Table 34.2 / Inhaled particulate materials, Absorption type F

  $fr = 1
  $sr = 30
  $ss =  0
  $fb = 0               # footnote*
  $sb = 0               # footnote*

$if parent
  ${Pt}/fA    = 0.01
  ${Pt}/fA_Re = fA_MaxValueOfIng    # footnote§
$else
  ${*}/fA     = fr * fA_MaxValueOfIng
  ${*}/fA_Re  =      fA_MaxValueOfIng
$endif

[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_79-Au.inc]
[include:element_78-Pt.inc]
[include:element_77-Ir.inc]
[include:element_76-Os.inc]
[include:element_75-Re.inc]
