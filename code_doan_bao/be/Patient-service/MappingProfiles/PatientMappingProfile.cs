using AutoMapper;
using Models.Dto;
using Patient_service.Models;
using Patient_service.Models.Dto;
using System.Linq; // Cần thiết cho các thao tác Select

namespace Patient_service.MappingProfiles
{
    public class PatientMappingProfile : Profile
    {
        public PatientMappingProfile()
        {
            // 1. Ánh xạ Patient (đã có)
            CreateMap<Patient, PatientDto>().ReverseMap();

            // ----------------------------------------------------
            // 🚨 BỔ SUNG ÁNH XẠ ĐỂ KHẮC PHỤC LỖI AutoMapperMappingException
            CreateMap<LabTest, LabTestDto>().ReverseMap();

            // 3. Ánh xạ MedicalRecord <--> MedicalRecordDto

            // Map Entity -> DTO (Cho đầu ra GET và trả về sau POST)
            CreateMap<MedicalRecord, MedicalRecordDto>()
                // Mối quan hệ N-N được xử lý tự động khi tên collection khớp (LabTests)
                // Tuy nhiên, nếu bạn muốn ánh xạ cả LabTestIds (nếu nó tồn tại trong DTO):
                .ForMember(dest => dest.LabTestIds, // Nếu bạn có thuộc tính LabTestIds trong DTO Output
                           opt => opt.MapFrom(src => src.LabTests.Select(lt => lt.Id).ToList()))
                .ReverseMap(); // Ánh xạ ngược vẫn ổn nếu bạn không dùng nó cho POST/PUT phức tạp

            // 🚨 QUAN TRỌNG: Ánh xạ DTO -> Entity (Cho đầu vào POST/PUT)
            // Khi map DTO -> Entity, ta phải bỏ qua collection LabTests và chỉ ánh xạ LabTestIds.
            CreateMap<MedicalRecordDto, MedicalRecord>()
                .ForMember(dest => dest.LabTests, opt => opt.Ignore()) // Bỏ qua collection khi map từ DTO
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // Bỏ qua ID kh

            // Ánh xạ Entity -> DTO (Cho đầu ra GET và trả về sau POST)
            CreateMap<MedicalRecord, MedicalRecordDto>()
                // Ánh xạ Collection LabTests. AutoMapper sẽ sử dụng map LabTest -> LabTestDto ở trên.
                // Nếu DTO của bạn có thuộc tính 'LabTests' là List<LabTestDto>, không cần ForMember.
                .ReverseMap(); // Sử dụng ReverseMap nếu DTO và Entity có cấu trúc tương tự

            // 4. Nếu DTO tạo mới (MedicalRecordCreateDto) có cấu trúc khác, cần thêm:
            // Ví dụ: Tạo map từ DTO -> Entity cho phương thức POST/PUT
            // CreateMap<MedicalRecordDto, MedicalRecord>()
            //     .ForMember(dest => dest.Id, opt => opt.Ignore()) // Bỏ qua ID khi tạo mới
            //     .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()); // Bỏ qua CreatedAt khi map từ DTO
        }
    }
}