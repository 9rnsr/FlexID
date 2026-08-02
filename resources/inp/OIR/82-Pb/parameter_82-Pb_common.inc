[parameter]

# ICRP Publ.137 p.231 Para.484 / 鉛子孫元素としてのポロニウム
# > (484) ... polonium as a progeny transferred to blood from either the respiratory
# > or alimentary tract is assigned to Plasma 2. ...
# > (484) ... 呼吸器系または消化器系から血液に移行する子孫核種としてのポロニウムは、
# > 血漿2に割り当てられます。 ...

  ${Po}/Blood       = 'Plasma2'

  ${Bi,Pb,Tl}/Blood = 'Plasma'
  ${Hg}/Blood       = 'Plasma1'
  ${Au}/Blood       = 'Blood1'

  ${Po}/fA_MaxValueOfIng = 0.1          # ICRP Publ.137 p.279 Table 11.2 / Ingested materials, All chemical forms
  ${Bi}/fA_MaxValueOfIng = 0.05         # ICRP Publ.137 p.253 Table 10.2 / Ingested materials, All forms
  ${Pb}/fA_MaxValueOfIng = 0.2          # ICRP Publ.137 p.220 Table  9.2 / Ingested materials, All forms
  ${Tl}/fA_MaxValueOfIng = 0.99 # ≒1   # ICRP Publ.151 p.344 Table 37.2 / Ingested materials, All forms
  ${Hg}/fA_MaxValueOfIng = 0.1          # ICRP Publ.151 p.327 Table 36.3 / Ingested materials, All forms
  ${Au}/fA_MaxValueOfIng = 0.1          # ICRP Publ.151 p.312 Table 35.2 / Ingested materials, All forms
