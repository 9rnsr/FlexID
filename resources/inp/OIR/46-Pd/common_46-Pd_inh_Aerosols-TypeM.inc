[include:common_46-Pd_parameter.inc]

[parameter]
  # ICRP Publ.151 p.220 Table 24.2 / Inhaled particulate materials, Absorption type M

  $fr = 0.2
  $sr = 3
  $ss = 0.005
  $fb = 0                   # footnote*
  $sb = 0                   # footnote*

$if parent
  ${Pd}/fA    = 0.001
  ${Pd}/fA_Re = fA_MaxValueOfIng    # footnote§
$else
  ${*}/fA     = fr * fA_MaxValueOfIng
  ${*}/fA_Re  =      fA_MaxValueOfIng
$endif


[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_47-Ag.inc]
[include:element_46-Pd.inc]
[include:element_45-Rh.inc]
