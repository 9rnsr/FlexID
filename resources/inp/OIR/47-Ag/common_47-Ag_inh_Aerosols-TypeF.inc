[include:common_47-Ag_parameter.inc]

[parameter]
  # ICRP Publ.151 p.233 Table 25.3 / Inhaled particulate materials, Absorption type F

  $fr = 1
  $sr = 1
  $ss = 0
  $fb = 0                   # footnote*
  $sb = 0                   # footnote*

$if parent
  ${Ag}/fA    = 0.05
  ${Ag}/fA_Re = fA_MaxValueOfIng    # footnote§
$else
  ${*}/fA     = fr * fA_MaxValueOfIng
  ${*}/fA_Re  =      fA_MaxValueOfIng
$endif


[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_49-In.inc]
[include:element_48-Cd.inc]
[include:element_47-Ag.inc]
[include:element_46-Pd.inc]
[include:element_45-Rh.inc]
