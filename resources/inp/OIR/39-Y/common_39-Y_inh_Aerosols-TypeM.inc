[include:common_39-Y_parameter.inc]

[parameter]
  # ICRP Publ.134 p.242 Table 11.2 / Inhaled particulate materials, Absorption type M

  $fr = 0.2
  $sr = 1
  $ss = 0.005
  $fb = 0           # footnote*
  $sb = 0           # footnote*

$if parent
  ${Y}/fA    = 2E-5
  ${Y}/fA_Re = fA_MaxValueOfIng     # footnote**
$else
  ${*}/fA    = fr * fA_MaxValueOfIng
  ${*}/fA_Re =      fA_MaxValueOfIng
$endif

[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_41-Nb.inc]
[include:element_40-Zr.inc]
[include:element_39-Y.inc]
[include:element_38-Sr.inc]
