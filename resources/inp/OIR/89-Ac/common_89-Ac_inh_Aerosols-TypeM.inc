[include:common_89-Ac_parameter.inc]

[parameter]
  # ICRP Publ.141 p.240 Table 19.2 / Inhaled particulate materials, Absorption type M

  $fr = 0.2
  $sr = 0.4
  $ss = 0.005
  $fb = 0.002       # footnote*
  $sb = 0           # footnote*

$if parent
  ${Ac}/fA     = 1E-4
  ${Ac}/fA_Re  = fA_MaxValueOfIng   # footnote¶
$else
  ${^Rn}/fA    = fr * fA_MaxValueOfIng
  ${^Rn}/fA_Re =      fA_MaxValueOfIng
$endif

[include:../share/common_intake_inh_Aerosols.inc]

[include:../share/common_HRTM.inc]
[include:../share/common_HATM+UB.inc]

[include:element_90-Th.inc]
[include:element_89-Ac.inc]
[include:element_88-Ra.inc]
[include:element_87-Fr.inc]
[include:element_86-Rn.inc]
[include:element_85-At.inc]
[include:element_84-Po.inc]
[include:element_83-Bi.inc]
[include:element_82-Pb.inc]
[include:element_81-Tl.inc]
