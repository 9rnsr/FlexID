[include:common_37-Rb_parameter.inc]

[parameter]
  # ICRP Publ.151 p.202 Table 22.2 / Inhaled particulate materials, Absorption type M

  $fr = 0.2
  $sr = 3
  $ss = 0.005
  $fb = 0                   # footnote*
  $sb = 0                   # footnote*

$if parent
  ${Rb}/fA    = 0.2
  ${Rb}/fA_Re = fA_MaxValueOfIng    # footnote§
$else
  ${*}/fA     = fr * fA_MaxValueOfIng
  ${*}/fA_Re  =      fA_MaxValueOfIng
$endif


[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_38-Sr.inc]
[include:element_37-Rb.inc]
[include:element_36-Kr.inc]
