[include:common_28-Ni_parameter.inc]

[parameter]
  # ICRP Publ.151 p.138 Table 15.3 / Inhaled particulate materials, Absorption type F

  $fr = 1
  $sr = 3
  $ss = 0
  $fb = 0                   # footnote*
  $sb = 0                   # footnote*

$if parent
  ${Ni}/fA    = 0.05
  ${Ni}/fA_Re = fA_MaxValueOfIng    # footnote¶
$else
  ${*}/fA     = fr * fA_MaxValueOfIng
  ${*}/fA_Re  =      fA_MaxValueOfIng
$endif


[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_29-Cu.inc]
[include:element_28-Ni.inc]
[include:element_27-Co.inc]
