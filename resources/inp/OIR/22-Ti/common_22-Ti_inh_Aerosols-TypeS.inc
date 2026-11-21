[include:common_22-Ti_parameter.inc]

[parameter]
  # ICRP Publ.151 p.95 Table 11.2 / Inhaled particulate materials, Absorption type S

  $fr = 0.01
  $sr = 3
  $ss = 1E-4
  $fb = 0               # footnote*
  $sb = 0               # footnote*

$if parent
  ${Ti}/fA    = 1E-5
  ${Ti}/fA_Re = fA_MaxValueOfIng    # footnote§
$else
  ${*}/fA     = fr * fA_MaxValueOfIng
  ${*}/fA_Re  =      fA_MaxValueOfIng
$endif


[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_22-Ti.inc]
[include:element_21-Sc.inc]
