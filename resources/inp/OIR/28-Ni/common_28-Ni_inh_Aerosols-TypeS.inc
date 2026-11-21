[include:common_28-Ni_parameter.inc]

[parameter]
  # ICRP Publ.151 p.138 Table 15.3 / Inhaled particulate materials, Absorption type S

  $fr = 0.01
  $sr = 3
  $ss = 1E-4
  $fb = 0                   # footnote*
  $sb = 0                   # footnote*

$if parent
  ${Ni}/fA    = 5E-4
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
