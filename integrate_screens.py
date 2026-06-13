import os
import re

MAPPING = [
    {
        "src": "akademik_takvim_ok_obs",
        "dest": "Views/Academic/Calendar.cshtml",
        "layout": True,
        "title": "Academic Calendar"
    },
    {
        "src": "ayarlar_ok_obs",
        "dest": "Views/Settings/Index.cshtml",
        "layout": True,
        "title": "Settings"
    },
    {
        "src": "bt_destek_ok_obs",
        "dest": "Views/Support/ITSupport.cshtml",
        "layout": True,
        "title": "IT Support"
    },
    {
        "src": "dan_man_giri_i_ok_obs",
        "dest": "Views/AdvisorAuth/Login.cshtml",
        "layout": False,
        "title": "Advisor Login"
    },
    {
        "src": "dan_man_kayd_ok_obs",
        "dest": "Views/AdvisorAuth/Register.cshtml",
        "layout": False,
        "title": "Advisor Register"
    },
    {
        "src": "dan_man_paneli_ok_obs",
        "dest": "Views/Advisor/Dashboard.cshtml",
        "layout": True,
        "title": "Advisor Dashboard"
    },
    {
        "src": "dashboard_ok_obs",
        "dest": "Views/Dashboard/Index.cshtml",
        "layout": True,
        "title": "Dashboard"
    },
    {
        "src": "ders_kay_t_ok_obs",
        "dest": "Views/Registration/Index.cshtml",
        "layout": True,
        "title": "Course Registration"
    },
    {
        "src": "ders_onay_merkezi_ok_obs",
        "dest": "Views/Advisor/CourseApproval.cshtml",
        "layout": True,
        "title": "Course Approval Center"
    },
    {
        "src": "ders_program_ok_obs",
        "dest": "Views/Academic/Schedule.cshtml",
        "layout": True,
        "title": "Weekly Schedule"
    },
    {
        "src": "devams_zl_k_takibi_ok_obs",
        "dest": "Views/Academic/Attendance.cshtml",
        "layout": True,
        "title": "Attendance Tracking"
    },
    {
        "src": "giri_yap_ok_obs",
        "dest": "Views/Auth/Login.cshtml",
        "layout": False,
        "title": "Login"
    },
    {
        "src": "gizlilik_politikas_ok_obs",
        "dest": "Views/Support/PrivacyPolicy.cshtml",
        "layout": True,
        "title": "Privacy Policy"
    },
    {
        "src": "har_deme_ok_obs",
        "dest": "Views/Registration/Tuition.cshtml",
        "layout": True,
        "title": "Tuition Payment"
    },
    {
        "src": "ho_geldiniz_ok_obs",
        "dest": "Views/Home/Welcome.cshtml",
        "layout": False,
        "title": "Welcome"
    },
    {
        "src": "i_leti_im_ok_obs",
        "dest": "Views/Support/Contact.cshtml",
        "layout": True,
        "title": "Contact Support"
    },
    {
        "src": "kay_t_ol_ok_obs",
        "dest": "Views/Auth/Register.cshtml",
        "layout": False,
        "title": "Register"
    },
    {
        "src": "kullan_m_artlar_ok_obs",
        "dest": "Views/Support/TermsOfUse.cshtml",
        "layout": True,
        "title": "Terms of Use"
    },
    {
        "src": "mesajlar_ve_duyurular_ok_obs",
        "dest": "Views/News/Index.cshtml",
        "layout": True,
        "title": "Messages & Announcements"
    },
    {
        "src": "not_durumu_ok_obs",
        "dest": "Views/Academic/Grades.cshtml",
        "layout": True,
        "title": "Grades Status"
    },
    {
        "src": "renci_el_kitab_ok_obs",
        "dest": "Views/Support/StudentHandbook.cshtml",
        "layout": True,
        "title": "Student Handbook"
    },
    {
        "src": "renci_y_netimi_ok_obs",
        "dest": "Views/Advisor/StudentManagement.cshtml",
        "layout": True,
        "title": "Student Management"
    },
    {
        "src": "transkript_ok_obs",
        "dest": "Views/Academic/Transkript.cshtml",
        "layout": True,
        "title": "Transcript"
    },
    {
        "src": "yard_m_merkezi_ok_obs",
        "dest": "Views/Support/HelpCenter.cshtml",
        "layout": True,
        "title": "Help Center"
    },
    # New Screens
    {
        "src": "bildirim_merkezi_ok_obs",
        "dest": "Views/Dashboard/Notifications.cshtml",
        "layout": True,
        "title": "Notifications"
    },
    {
        "src": "ders_materyalleri_ok_obs",
        "dest": "Views/Academic/Materials.cshtml",
        "layout": True,
        "title": "Course Materials"
    },
    {
        "src": "hakk_m_zda_ok_obs",
        "dest": "Views/Home/About.cshtml",
        "layout": True,
        "title": "About Us"
    },
    {
        "src": "ifremi_unuttum_ok_obs",
        "dest": "Views/Auth/ForgotPassword.cshtml",
        "layout": False,
        "title": "Forgot Password"
    },
    {
        "src": "profil_bilgilerim_ok_obs",
        "dest": "Views/Settings/Profile.cshtml",
        "layout": True,
        "title": "My Profile"
    },
    {
        "src": "sayfa_bulunamad_ok_obs",
        "dest": "Views/Home/NotFound.cshtml",
        "layout": True,
        "title": "Page Not Found"
    }
]

def process_file(mapping_item):
    src_dir = os.path.join("screens", mapping_item["src"])
    code_html_path = os.path.join(src_dir, "code.html")
    if not os.path.exists(code_html_path):
        print(f"Warning: File not found: {code_html_path}")
        return
    
    with open(code_html_path, "r", encoding="utf-8") as f:
        html_content = f.read()

    # Escape @ as @@ in the HTML content to avoid Razor interpretation
    html_content = html_content.replace("@", "@@")

    # Determine output path and ensure dirs exist
    dest_path = mapping_item["dest"]
    os.makedirs(os.path.dirname(dest_path), exist_ok=True)

    if mapping_item["layout"]:
        # Find <main> ... </main>
        main_match = re.search(r"<main[^>]*>(.*?)</main>", html_content, re.DOTALL)
        if main_match:
            main_content = main_match.group(1).strip()
        else:
            body_match = re.search(r"<body[^>]*>(.*?)</body>", html_content, re.DOTALL)
            main_content = body_match.group(1).strip() if body_match else html_content

        # Strip duplicate headers/footers
        main_content = re.sub(r"<header[^>]*>.*?</header>", "", main_content, flags=re.DOTALL)
        main_content = re.sub(r"<footer[^>]*>.*?</footer>", "", main_content, flags=re.DOTALL)
        main_content = main_content.strip()

        # Find specific styles in the head (ignoring common tailwind configs/fonts)
        style_matches = re.findall(r"<style[^>]*>(.*?)</style>", html_content, re.DOTALL)
        custom_styles = []
        for s in style_matches:
            if "body {" in s or "glass-panel" in s or "glass-card" in s:
                lines = s.split("\n")
                filtered_lines = []
                skip = False
                for line in lines:
                    if "body {" in line or ".glass-panel {" in line or ".glass-card {" in line:
                        skip = True
                    elif "}" in line and skip:
                        skip = False
                        continue
                    if not skip:
                        filtered_lines.append(line)
                clean_s = "\n".join(filtered_lines).strip()
                if clean_s:
                    custom_styles.append(clean_s)
            else:
                custom_styles.append(s.strip())

        # Find scripts
        script_matches = re.findall(r"<script[^>]*>(.*?)</script>", html_content, re.DOTALL)
        custom_scripts = []
        for script in script_matches:
            if "tailwind.config" not in script and "cdn.tailwindcss.com" not in script:
                custom_scripts.append(script.strip())

        # Write output Razor view
        view_content = f'@{{ ViewData["Title"] = "{mapping_item["title"]}"; }}\n\n'
        
        if custom_styles:
            view_content += "@section Styles {\n<style>\n" + "\n\n".join(custom_styles) + "\n</style>\n}\n\n"
            
        # Standardize container to fit nicely under Layout
        view_content += f'<div class="flex-grow p-gutter relative overflow-hidden w-full">\n'
        view_content += main_content
        view_content += "\n</div>\n"

        if custom_scripts:
            view_content += "\n@section Scripts {\n<script>\n" + "\n\n".join(custom_scripts) + "\n</script>\n}\n"

    else:
        view_content = '@{ Layout = null; }\n' + html_content

    # Write file
    with open(dest_path, "w", encoding="utf-8") as f:
        f.write(view_content)
    print(f"Successfully integrated {mapping_item['src']} -> {dest_path}")

def main():
    for item in MAPPING:
        process_file(item)

if __name__ == "__main__":
    main()
