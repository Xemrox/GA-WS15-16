find_files = $(wildcard $(dir)/*.cs)
dirs := . Config Helper Folding SilentStatistics
files := $(foreach dir,$(dirs),$(find_files))

target = mono/GA.exe
flags = 
aotflag = --aot

all: clean
	mcs $(files) -out:$(target) $(flags)

aot: all
	mono $(aotflag) $(target)
	chmod +x $(target).so

run: all
	mono $(target)
	
clean:
	@rm -f $(target)