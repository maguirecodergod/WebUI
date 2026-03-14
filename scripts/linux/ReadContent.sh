#!/bin/bash

# Khai báo danh sách các thư mục cần xử lý
target_dirs=(
    "src/modules/DomainDrivenDesign"
    "src/modules/Core"
    "src/modules/Caching"
    "src/modules/MultiTenancy"
    "src/modules/EfCore"
    "test"
)

# Lặp qua từng thư mục trong danh sách
for target_dir in "${target_dirs[@]}"; do
    # Kiểm tra nếu thư mục không tồn tại
    if [ ! -d "$target_dir" ]; then
        echo "Directory '$target_dir' does not exist. Skipping."
        continue
    fi

    # Tạo tên file output với timestamp cho từng thư mục
    timestamp=$(date +"%Y%m%d_%H%M%S")
    output_file="$(basename "$target_dir")_$timestamp.txt"

    rm -f "$output_file"

    echo "Processing directory: $target_dir" > "$output_file"
    echo "" >> "$output_file"

    # Tìm tất cả các file trong thư mục, loại bỏ các file trong bin và obj
    find "$target_dir" -type f ! -path "*/bin/*" ! -path "*/logs/*" ! -path "*/obj/*" | while read file; do
        # Ghi trực tiếp nội dung của file vào output mà không ghi thông báo "Reading: ..."
        echo "===== FILE: $file =====" >> "$output_file"
        cat "$file" >> "$output_file"
        echo -e "\n\n" >> "$output_file"
    done

    # Thêm dấu cách giữa các thư mục
    echo "===== End of Directory: $target_dir =====" >> "$output_file"
    echo "" >> "$output_file"

    echo "Done! Output saved to $output_file"
done