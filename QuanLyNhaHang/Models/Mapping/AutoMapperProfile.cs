using AutoMapper;
using System;
using System.Globalization;

namespace QuanLyNhaHang.Models.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<PhieuNhap, PhieuNhapMap>()
            .ForMember(dest => dest.Idnv, opt => opt.MapFrom(src => src.Idnv.ToString()))
            .ForMember(dest => dest.NgayNhap, opt => opt.MapFrom(src => src.NgayNhap.ToString()))
            .ForMember(dest => dest.NgayHd, opt => opt.MapFrom(src => src.NgayHd.ToString()))
           .ForMember(dest => dest.Idncc, opt => opt.MapFrom(src => src.Idncc.ToString()));
            CreateMap<PhieuNhapMap, PhieuNhap>()
            .ForMember(dest => dest.Idnv, opt => opt.MapFrom(src => src.Idnv != null ? long.Parse(src.Idnv) : (long?)null))
            .ForMember(dest => dest.Idncc, opt => opt.MapFrom(src => src.Idncc != null ? long.Parse(src.Idncc) : (long?)null))
            .ForMember(dest => dest.NgayNhap, opt => opt.MapFrom(src => src.NgayNhap != "" ? DateTime.ParseExact(src.NgayNhap, "dd-MM-yyyy", CultureInfo.InvariantCulture) : (DateTime?)null))
            .ForMember(dest => dest.NgayHd, opt => opt.MapFrom(src => src.NgayHd != "" ? DateTime.ParseExact(src.NgayHd, "dd-MM-yyyy", CultureInfo.InvariantCulture) : (DateTime?)null));
            CreateMap<ChiTietPhieuNhap, ChiTietPhieuNhapMap>()
            .ForMember(dest => dest.Idpn, opt => opt.MapFrom(src => src.Idpn.ToString()))
            .ForMember(dest => dest.Idhh, opt => opt.MapFrom(src => src.Idhh.ToString()))
            .ForMember(dest => dest.SoLuong, opt => opt.MapFrom(src => src.SoLuong.ToString()))
            .ForMember(dest => dest.Hsd, opt => opt.MapFrom(src => src.Hsd.ToString()))
            .ForMember(dest => dest.Nsx, opt => opt.MapFrom(src => src.Nsx.ToString()))
            .ForMember(dest => dest.Gia, opt => opt.MapFrom(src => src.Gia.ToString()));
            CreateMap<ChiTietPhieuNhapMap, ChiTietPhieuNhap>()
            .ForMember(dest => dest.Idpn, opt => opt.MapFrom(src => src.Idpn != null ? long.Parse(src.Idpn) : (long?)null))
            .ForMember(dest => dest.Idhh, opt => opt.MapFrom(src => src.Idhh != null ? long.Parse(src.Idhh) : (long?)null))
            .ForMember(dest => dest.SoLuong, opt => opt.MapFrom(src => src.SoLuong != "" ? double.Parse(src.SoLuong.Replace(",", "")) : (double?)null))
            .ForMember(dest => dest.Gia, opt => opt.MapFrom(src => src.Gia != "" ? double.Parse(src.Gia.Replace(",", "")) : (double?)null))
            .ForMember(dest => dest.Hsd, opt => opt.MapFrom(src => src.Hsd != "" ? DateTime.ParseExact(src.Hsd, "dd-MM-yyyy", CultureInfo.InvariantCulture) : (DateTime?)null))
            .ForMember(dest => dest.Nsx, opt => opt.MapFrom(src => src.Nsx != "" ? DateTime.ParseExact(src.Nsx, "dd-MM-yyyy", CultureInfo.InvariantCulture) : (DateTime?)null));
            CreateMap<PhieuXuat, PhieuXuatMap>()
            .ForMember(dest => dest.Idkh, opt => opt.MapFrom(src => src.Idkh.ToString()))
            .ForMember(dest => dest.NgayHd, opt => opt.MapFrom(src => src.NgayHd.ToString()))
            .ForMember(dest => dest.NgayTao, opt => opt.MapFrom(src => src.NgayTao.ToString()));
            CreateMap<PhieuXuatMap, PhieuXuat>()
            .ForMember(dest => dest.Idkh, opt => opt.MapFrom(src => src.Idkh != null ? long.Parse(src.Idkh) : (long?)null))
            .ForMember(dest => dest.NgayHd, opt => opt.MapFrom(src => src.NgayHd != "" ? DateTime.ParseExact(src.NgayHd, "dd-MM-yyyy", CultureInfo.InvariantCulture) : (DateTime?)null))

            .ForMember(dest => dest.NgayTao, opt => opt.MapFrom(src => src.NgayHd != "" ? DateTime.ParseExact(src.NgayTao, "dd-MM-yyyy", CultureInfo.InvariantCulture) : (DateTime?)null));

        }
    }
}
